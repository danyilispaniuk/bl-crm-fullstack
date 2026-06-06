import { Component, Input, HostListener } from '@angular/core';
import { DatePipe, NgClass } from '@angular/common';
import { Contract } from '../../services/contracts.service';

@Component({
  selector: 'app-contract-card',
  imports: [DatePipe, NgClass],
  templateUrl: './contract-card.html',
  styleUrl: './contract-card.scss'
})
export class ContractCardComponent {
  @Input({ required: true }) contract!: Contract;
  isMenuOpen = false;

  @HostListener('document:click')
  onDocumentClick(): void {
    this.closeMenu();
  }

  toggleMenu(event: MouseEvent): void {
    event.stopPropagation();
    this.isMenuOpen = !this.isMenuOpen;
  }

  closeMenu(): void {
    this.isMenuOpen = false;
  }

  isContractActive(validityDateStr: string, endDateStr?: string): boolean {
    const today = new Date();
    today.setHours(0, 0, 0, 0);

    if (endDateStr) {
      const endDate = new Date(endDateStr);
      if (endDate < today) return false;
    }

    const validityDate = new Date(validityDateStr);
    return validityDate >= today;
  }
}
