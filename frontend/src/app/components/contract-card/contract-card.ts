import { Component, Input, Output, EventEmitter, HostListener } from '@angular/core';
import { DatePipe } from '@angular/common';
import { Contract } from '../../services/contracts.service';

@Component({
  selector: 'app-contract-card',
  imports: [DatePipe],
  templateUrl: './contract-card.html',
  styleUrl: './contract-card.scss'
})
export class ContractCardComponent {
  @Input({ required: true }) contract!: Contract;
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

  closeMenu(): void {
    this.isMenuOpen = false;
  }
}
