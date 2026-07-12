import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { NgClass } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ObserveVisibilityDirective } from '../../../shared/directives/observe-visibility.directive';
import { ContactApiService }  from '../../../core/services/contact-api.service';
import { ToastService }       from '../../../core/services/toast.service';
import { ContactInfoItem, SocialLink, SubmitState } from '../../../core/models/portfolio.models';
import { LucideAngularModule, Mail, Github, Linkedin, Globe, Clock } from 'lucide-angular';

const MESSAGE_MAX_LENGTH = 500;

@Component({
  selector: 'app-contact',
  standalone: true,
  imports: [NgClass, ReactiveFormsModule, ObserveVisibilityDirective, LucideAngularModule],
  templateUrl: './contact.component.html',
  styleUrl: './contact.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ContactComponent {
  private readonly fb          = inject(FormBuilder);
  private readonly contactApi  = inject(ContactApiService);
  private readonly toastSvc    = inject(ToastService);

  readonly headerVisible = signal(false);
  readonly leftVisible   = signal(false);
  readonly formVisible   = signal(false);
  readonly submitState   = signal<SubmitState>('idle');
  readonly messageMaxLength = MESSAGE_MAX_LENGTH;

  readonly Mail = Mail;
  readonly Github = Github;
  readonly Linkedin = Linkedin;
  readonly Globe = Globe;
  readonly Clock = Clock;
  
  readonly contactForm = this.fb.nonNullable.group({
    name:    ['', [Validators.required, Validators.minLength(2)]],
    email:   ['', [Validators.required, Validators.email]],
    subject: ['', [Validators.required, Validators.minLength(3)]],
    message: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(MESSAGE_MAX_LENGTH)]],
  });

  private readonly messageLengthTrigger = signal(0);

  readonly messageLength = computed(() => {
    this.messageLengthTrigger();
    return this.contactForm.controls.message.value.length;
  });

  bumpMessageLength(): void {
    this.messageLengthTrigger.update(v => v + 1);
  }

  readonly infoItems: ContactInfoItem[] = [
    {
      label: 'Email',
      value: 'rajpootprashant1604@gmail.com',
      icon: this.Mail,
    },
    {
      label: 'Location',
      value: 'Aligarh, India',
      icon: this.Globe,
    },
    {
      label: 'Response time',
      value: 'Usually within 24 hours',
      icon: this.Clock,
    },
  ];

  readonly socialLinks: SocialLink[] = [
    {
      id: 'github', label: 'GitHub', href: 'https://github.com/predator1604',
      icon: this.Github,
    },
    {
      id: 'linkedin', label: 'LinkedIn', href: 'https://www.linkedin.com/in/prashant1604',
      icon: this.Linkedin,
    },
    {
      id: 'email', label: 'Email', href: 'https://mail.google.com/mail/?view=cm&fs=1&to=rajpootprashant1604@gmail.com',
      icon: this.Mail,
    },
  ];

  fieldHasError(field: 'name' | 'email' | 'subject' | 'message'): boolean {
    const c = this.contactForm.controls[field];
    return c.invalid && (c.dirty || c.touched);
  }

  fieldErrorMessage(field: 'name' | 'email' | 'subject' | 'message'): string {
    const errors = this.contactForm.controls[field].errors;
    if (!errors) return '';
    if (errors['required'])  return 'This field is required.';
    if (errors['email'])     return 'Enter a valid email address.';
    if (errors['minlength']) return `Must be at least ${errors['minlength'].requiredLength} characters.`;
    if (errors['maxlength']) return `Must be under ${MESSAGE_MAX_LENGTH} characters.`;
    return 'Invalid value.';
  }

  // ── Real API call ──────────────────────────────────────────────────────────
  onSubmit(): void {
    if (this.contactForm.invalid) {
      this.contactForm.markAllAsTouched();
      return;
    }

    this.submitState.set('submitting');
    const { name, email, subject, message } = this.contactForm.getRawValue();

    this.contactApi.send({ name, email, subject, message }).subscribe({
      next: (res) => {
        this.submitState.set('success');
        this.contactForm.reset();
        this.messageLengthTrigger.update(v => v + 1);
        this.toastSvc.success('Message sent!', res.confirmationText);
        setTimeout(() => this.submitState.set('idle'), 4000);
      },
      error: (err: Error) => {
        this.submitState.set('error');
        this.toastSvc.error('Failed to send', err.message);
        setTimeout(() => this.submitState.set('idle'), 3000);
      },
    });
  }

  trackInfo(_: number, item: ContactInfoItem): string { return item.label; }
  trackSocial(_: number, link: SocialLink): string    { return link.id; }
}
