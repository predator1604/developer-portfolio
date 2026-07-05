import { Injectable, signal } from '@angular/core';
import { Toast } from '../models/portfolio.models';

@Injectable({ providedIn: 'root' })
export class ToastService {
  readonly toasts = signal<Toast[]>([]);

  success(title: string, message?: string, duration = 4000): void {
    this.push({ type: 'success', title, message, duration });
  }

  error(title: string, message?: string, duration = 5000): void {
    this.push({ type: 'error', title, message, duration });
  }

  info(title: string, message?: string, duration = 3500): void {
    this.push({ type: 'info', title, message, duration });
  }

  dismiss(id: string): void {
    this.toasts.update(ts => ts.filter(t => t.id !== id));
  }

  private push(toast: Omit<Toast, 'id'>): void {
    const id = crypto.randomUUID();
    this.toasts.update(ts => [...ts, { ...toast, id }]);
    setTimeout(() => this.dismiss(id), toast.duration);
  }
}
