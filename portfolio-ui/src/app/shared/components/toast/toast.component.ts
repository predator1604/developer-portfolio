import {
  ChangeDetectionStrategy,
  Component,
  inject,
} from '@angular/core';
import { NgClass } from '@angular/common';
import { ToastService } from '../../../core/services/toast.service';
import { Toast } from '../../../core/models/portfolio.models';

@Component({
  selector: 'app-toast',
  standalone: true,
  imports: [NgClass],
  template: `
    <div class="toast-container" role="region" aria-label="Notifications" aria-live="polite">
      @for (toast of toastSvc.toasts(); track toast.id) {
        <div
          class="toast"
          [class.toast--success]="toast.type === 'success'"
          [class.toast--error]="toast.type === 'error'"
          [class.toast--info]="toast.type === 'info'"
          role="alert"
        >
          <span class="toast__icon" aria-hidden="true">
            @if (toast.type === 'success') { ✓ }
            @if (toast.type === 'error')   { ✕ }
            @if (toast.type === 'info')    { i }
          </span>
          <div class="toast__body">
            <p class="toast__title">{{ toast.title }}</p>
            @if (toast.message) {
              <p class="toast__msg">{{ toast.message }}</p>
            }
          </div>
          <button
            class="toast__close"
            (click)="toastSvc.dismiss(toast.id)"
            aria-label="Dismiss notification"
          >✕</button>
        </div>
      }
    </div>
  `,
  styles: [`
    .toast-container {
      position: fixed;
      bottom: 28px;
      left: 50%;
      transform: translateX(-50%);
      z-index: 9999;
      display: flex;
      flex-direction: column;
      gap: 10px;
      pointer-events: none;
      width: min(440px, 92vw);
    }

    .toast {
      display: flex;
      align-items: flex-start;
      gap: 12px;
      padding: 14px 16px;
      border-radius: 10px;
      border: 1px solid;
      backdrop-filter: blur(12px);
      pointer-events: auto;
      animation: toastIn 0.3s cubic-bezier(0.34,1.56,0.64,1) both;

      &--success {
        background: rgba(6, 18, 12, 0.92);
        border-color: rgba(34, 197, 94, 0.3);
        .toast__icon { background: rgba(34,197,94,0.15); color: #22c55e; }
      }
      &--error {
        background: rgba(18, 6, 6, 0.92);
        border-color: rgba(248, 113, 113, 0.3);
        .toast__icon { background: rgba(248,113,113,0.15); color: #f87171; }
      }
      &--info {
        background: rgba(6, 10, 18, 0.92);
        border-color: rgba(99, 102, 241, 0.3);
        .toast__icon { background: rgba(99,102,241,0.15); color: #6366f1; }
      }
    }

    .toast__icon {
      width: 26px;
      height: 26px;
      border-radius: 50%;
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 11px;
      font-weight: 700;
      flex-shrink: 0;
    }

    .toast__body { flex: 1; }

    .toast__title {
      font-size: 13px;
      font-weight: 600;
      color: #f0f0f8;
      line-height: 1.4;
    }

    .toast__msg {
      font-size: 12px;
      color: #6b6b82;
      margin-top: 3px;
      line-height: 1.5;
    }

    .toast__close {
      background: none;
      border: none;
      color: #3a3a52;
      font-size: 12px;
      cursor: pointer;
      padding: 2px;
      flex-shrink: 0;
      transition: color 0.2s;
      &:hover { color: #6b6b82; }
    }

    @keyframes toastIn {
      from { opacity: 0; transform: translateY(12px) scale(0.95); }
      to   { opacity: 1; transform: translateY(0) scale(1); }
    }

    @media (prefers-reduced-motion: reduce) {
      .toast { animation: none; }
    }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ToastComponent {
  readonly toastSvc = inject(ToastService);

  trackToast(_: number, toast: Toast): string {
    return toast.id;
  }
}
