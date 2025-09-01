import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../services/auth';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './navbar.html',
  styleUrls: ['./navbar.css']
})
export class NavbarComponent implements OnInit {
  public username: string | null = null;
  public isDropdownVisible = false;

  constructor(private authService: AuthService) { }

  ngOnInit(): void {
    this.username = this.authService.getUsername();
  }

  toggleDropdown(): void {
    this.isDropdownVisible = !this.isDropdownVisible;
  }

  logout(): void {
    this.authService.logout();
  }
}
