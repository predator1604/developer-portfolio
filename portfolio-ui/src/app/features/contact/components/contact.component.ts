import {
  ChangeDetectionStrategy,
  Component,
  computed,
  inject,
  signal,
} from '@angular/core';
import { NgClass } from '@angular/common';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ObserveVisibilityDirective } from '../../../shared/directives/observe-visibility.directive';
import { ContactApiService }  from '../../../core/services/contact-api.service';
import { ToastService }       from '../../../core/services/toast.service';
import {
  ContactInfoItem,
  SocialLink,
  SubmitState,
} from '../../../core/models/portfolio.models';

const MESSAGE_MAX_LENGTH = 500;

@Component({
  selector: 'app-contact',
  standalone: true,
  imports: [NgClass, ReactiveFormsModule, ObserveVisibilityDirective],
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
      iconPath: `<path d="M2 4l6 4 6-4M2 4v8a1 1 0 001 1h10a1 1 0 001-1V4M2 4a1 1 0 011-1h10a1 1 0 011 1" stroke="#6366f1" stroke-width="1.3" stroke-linecap="round" stroke-linejoin="round"/>`,
    },
    {
      label: 'Location',
      value: 'Aligarh, India',
      iconPath: `<path d="M8 1c-2.76 0-5 2.24-5 5 0 3.75 5 9 5 9s5-5.25 5-9c0-2.76-2.24-5-5-5z" stroke="#6366f1" stroke-width="1.3" stroke-linejoin="round"/><circle cx="8" cy="6" r="1.6" stroke="#6366f1" stroke-width="1.3"/>`,
    },
    {
      label: 'Response time',
      value: 'Usually within 24 hours',
      iconPath: `<circle cx="8" cy="8" r="7" stroke="#6366f1" stroke-width="1.3"/><path d="M8 4v4l3 2" stroke="#6366f1" stroke-width="1.3" stroke-linecap="round" stroke-linejoin="round"/>`,
    },
  ];

  readonly socialLinks: SocialLink[] = [
    {
      id: 'github', label: 'GitHub', href: 'https://github.com/predator1604',
      iconPath: `<path d="M8 1C4.13 1 1 4.13 1 8c0 3.09 2.01 5.71 4.79 6.64.35.06.48-.15.48-.34v-1.19c-1.95.42-2.36-.94-2.36-.94-.32-.81-.78-1.03-.78-1.03-.64-.43.05-.42.05-.42.7.05 1.07.72 1.07.72.62 1.07 1.63.76 2.03.58.06-.45.24-.76.44-.94-1.56-.18-3.2-.78-3.2-3.46 0-.76.27-1.39.72-1.88-.07-.18-.31-.89.07-1.85 0 0 .59-.19 1.92.72A6.67 6.67 0 018 4.79c.59 0 1.19.08 1.75.23 1.33-.9 1.92-.72 1.92-.72.38.96.14 1.67.07 1.85.45.49.72 1.12.72 1.88 0 2.69-1.64 3.28-3.2 3.46.25.22.48.65.48 1.31v1.94c0 .19.13.4.48.34C12.99 13.71 15 11.09 15 8c0-3.87-3.13-7-7-7z" fill="#c0c0d0"/>`,
    },
    {
      id: 'linkedin', label: 'LinkedIn', href: 'https://www.linkedin.com/in/prashant1604',
      iconPath: `<path d="M13.5 1h-11A1.5 1.5 0 001 2.5v11A1.5 1.5 0 002.5 15h11a1.5 1.5 0 001.5-1.5v-11A1.5 1.5 0 0013.5 1zM5 6.5v6M5 4.25a.75.75 0 110-1.5.75.75 0 010 1.5zM8 6.5v6M8 9c0-1.5 1-2.5 2.25-2.5S12.5 7.5 12.5 9v3.5" stroke="#c0c0d0" stroke-width="1.3" stroke-linecap="round" stroke-linejoin="round"/>`,
    },
    {
      id: 'email', label: 'Email', href: 'mailto:rajpootprashant1604@gmail.com',
      iconPath: `<path d="M2 4l6 4 6-4M2 4v8a1 1 0 001 1h10a1 1 0 001-1V4M2 4a1 1 0 011-1h10a1 1 0 011 1" stroke="#c0c0d0" stroke-width="1.3" stroke-linecap="round" stroke-linejoin="round"/>`,
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
