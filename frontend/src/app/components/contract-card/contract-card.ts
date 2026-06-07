import { Component, Input, Output, EventEmitter, HostListener, inject, PLATFORM_ID, OnInit } from '@angular/core';
import { isPlatformBrowser, DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Contract } from '../../services/contracts.service';

@Component({
  selector: 'app-contract-card',
  imports: [DatePipe, RouterLink],
  templateUrl: './contract-card.html',
  styleUrl: './contract-card.scss'
})
export class ContractCardComponent implements OnInit {
  private platformId = inject(PLATFORM_ID);

  @Input({ required: true }) contract!: Contract;
  @Output() delete = new EventEmitter<void>();
  @Output() hide = new EventEmitter<void>();
  isMenuOpen = false;
  isAdmin = false;
  isAdvisor = false;

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      const role = localStorage.getItem('role');
      this.isAdmin = role === 'Admin';
      this.isAdvisor = role === 'Advisor';
    }
  }

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

  onHideClick(event: MouseEvent): void {
    event.stopPropagation();
    this.closeMenu();
    this.hide.emit();
  }

  closeMenu(): void {
    this.isMenuOpen = false;
  }
}
