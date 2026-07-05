import {
  ChangeDetectionStrategy,
  Component,
  HostListener,
  PLATFORM_ID,
  inject,
  signal,
} from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-scroll-to-top',
  standalone: true,
  imports: [NgClass],
  template: `
    <button
      class="stt"
      [class.stt--visible]="isVisible()"
      type="button"
      (click)="scrollToTop()"
      aria-label="Scroll back to top"
      [attr.tabindex]="isVisible() ? 0 : -1"
    >
      <svg
        width="16"
        height="16"
        viewBox="0 0 16 16"
        fill="none"
        aria-hidden="true"
      >
        <path
          d="M8 13V3M3 8l5-5 5 5"
          stroke="currentColor"
          stroke-width="1.6"
          stroke-linecap="round"
          stroke-linejoin="round"
        />
      </svg>
    </button>
  `,
  styles: [`
    .stt {
      position: fixed;
      bottom: 32px;
      right: 32px;
      z-index: 200;
      width: 42px;
      height: 42px;
      border-radius: 10px;
      border: 1px solid rgba(99, 102, 241, 0.35);
      background: rgba(10, 10, 15, 0.85);
      color: #6366f1;
      display: flex;
      align-items: center;
      justify-content: center;
      cursor: pointer;
      backdrop-filter: blur(10px);
      opacity: 0;
      transform: translateY(12px) scale(0.9);
      pointer-events: none;
      transition:
        opacity 0.25s ease,
        transform 0.25s ease,
        border-color 0.2s,
        background 0.2s;

      &--visible {
        opacity: 1;
        transform: translateY(0) scale(1);
        pointer-events: auto;
      }

      &:hover {
        background: rgba(99, 102, 241, 0.15);
        border-color: rgba(99, 102, 241, 0.6);
        transform: translateY(-2px) scale(1);
      }

      &:active {
        transform: translateY(0) scale(0.96);
      }
    }

    @media (max-width: 480px) {
      .stt { bottom: 20px; right: 20px; }
    }

    @media (prefers-reduced-motion: reduce) {
      .stt { transition: opacity 0.2s; transform: none !important; }
    }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ScrollToTopComponent {
  private readonly platformId = inject(PLATFORM_ID);
  readonly isVisible = signal(false);

  private readonly THRESHOLD = 300;

  @HostListener('window:scroll')
  onScroll(): void {
    if (!isPlatformBrowser(this.platformId)) return;
    this.isVisible.set(window.scrollY > this.THRESHOLD);
  }

  scrollToTop(): void {
    if (!isPlatformBrowser(this.platformId)) return;
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }
}
