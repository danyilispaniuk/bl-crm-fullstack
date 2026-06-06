import { Injectable, signal } from '@angular/core';

export type ToastType = 'error' | 'success' | 'info' | 'warning';

export interface Toast {
  id: number;
  message: string;
  type: ToastType;
}

@Injectable({
  providedIn: 'root'
})
export class ToastService {
  private _counter = 0;
  toasts = signal<Toast[]>([]);

  show(message: string, type: ToastType = 'info', duration = 4000): void {
    const id = ++this._counter;
    this.toasts.update(current => [...current, { id, message, type }]);
    setTimeout(() => this.dismiss(id), duration);
  }

  error(message: string): void {
    this.show(message, 'error');
  }

  success(message: string): void {
    this.show(message, 'success');
  }

  warning(message: string): void {
    this.show(message, 'warning');
  }

  info(message: string): void {
    this.show(message, 'info');
  }

  dismiss(id: number): void {
    this.toasts.update(current => current.filter(t => t.id !== id));
  }
}
