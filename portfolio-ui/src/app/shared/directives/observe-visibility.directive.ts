import {
  Directive,
  ElementRef,
  OnDestroy,
  OnInit,
  PLATFORM_ID,
  inject,
  input,
  output,
} from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

/**
 * ObserveVisibilityDirective
 *
 * Emits (visible) once when the host element enters the viewport.
 * Optionally adds a CSS class and unobserves after the first trigger.
 *
 * Usage:
 *   <div appObserveVisibility visibleClass="is-visible" (visible)="onVisible()">
 */
@Directive({
  selector: '[appObserveVisibility]',
  standalone: true,
})
export class ObserveVisibilityDirective implements OnInit, OnDestroy {
  private readonly el = inject(ElementRef<HTMLElement>);
  private readonly platformId = inject(PLATFORM_ID);

  /** CSS class added when element becomes visible */
  readonly visibleClass = input<string>('is-visible');

  /** Intersection threshold 0–1 */
  readonly threshold = input<number>(0.15);

  /** Emits true when element enters the viewport */
  readonly visible = output<boolean>();

  private observer!: IntersectionObserver;

  ngOnInit(): void {
    if (!isPlatformBrowser(this.platformId)) return;

    this.observer = new IntersectionObserver(
      (entries) => {
        const entry = entries[0];
        if (entry.isIntersecting) {
          this.el.nativeElement.classList.add(this.visibleClass());
          this.visible.emit(true);
          // Unobserve after first trigger — animate once
          this.observer.unobserve(this.el.nativeElement);
        }
      },
      { threshold: this.threshold() }
    );

    this.observer.observe(this.el.nativeElement);
  }

  ngOnDestroy(): void {
    this.observer?.disconnect();
  }
}
