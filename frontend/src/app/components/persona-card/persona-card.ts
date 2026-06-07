import { Component, Input, Output, EventEmitter, HostListener, inject, PLATFORM_ID, OnInit } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { Client } from '../../services/clients.service';

@Component({
  selector: 'app-persona-card',
  imports: [RouterLink],
  templateUrl: './persona-card.html',
  styleUrl: './persona-card.scss'
})
export class PersonaCardComponent implements OnInit {
  private router = inject(Router);
  private platformId = inject(PLATFORM_ID);

  @Input({ required: true }) person!: Client;
  @Output() delete = new EventEmitter<void>();
  isMenuOpen = false;
  isAdmin = false;

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      this.isAdmin = localStorage.getItem('role') === 'Admin';
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

  onOpenDetailsClick(event: MouseEvent): void {
    event.stopPropagation();
    this.closeMenu();
    if (this.person.role === 'Client') {
      this.router.navigate(['/clients', this.person.id]);
    }
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
