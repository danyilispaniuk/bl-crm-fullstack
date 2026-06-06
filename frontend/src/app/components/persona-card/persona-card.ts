import { Component, Input, Output, EventEmitter, HostListener } from '@angular/core';
import { Client } from '../../services/clients.service';

@Component({
  selector: 'app-persona-card',
  templateUrl: './persona-card.html',
  styleUrl: './persona-card.scss'
})
export class PersonaCardComponent {
  @Input({ required: true }) client!: Client;
  @Output() delete = new EventEmitter<void>();
  isMenuOpen = false;

  @HostListener('document:click')
  onDocumentClick(): void {
    this.closeMenu();
  }

  toggleMenu(event: MouseEvent): void {
    event.stopPropagation();
    this.isMenuOpen = !this.isMenuOpen;
  }

  onDeleteClick(event: MouseEvent): void {
    event.stopPropagation();
    this.closeMenu();
    this.delete.emit();
  }

  // To close menu when clicking anywhere else
  closeMenu(): void {
    this.isMenuOpen = false;
  }
}
