import { Component, inject } from '@angular/core';
import { NgClass } from '@angular/common';
import { ToastService, Toast } from '../../services/toast.service';

@Component({
  selector: 'app-toast',
  imports: [NgClass],
  templateUrl: './toast.html',
  styleUrl: './toast.scss'
})
export class ToastComponent {
  toastService = inject(ToastService);

  get toasts() {
    return this.toastService.toasts();
  }

  dismiss(id: number) {
    this.toastService.dismiss(id);
  }
}
