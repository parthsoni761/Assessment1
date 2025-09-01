import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth';
import { LoginDto } from '../../models/auth';

import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.html',
  imports: [
    CommonModule, // For *ngIf
    FormsModule,  // For ngForm and [(ngModel)]
    RouterLink    // For the routerLink directive
  ],
  styleUrls: ['../auth.css']
})
export class LoginComponent {
  public model: LoginDto = {
    usernameOrEmail: '',
    password: ''
  };
  public errorMessage = '';

  constructor(private authService: AuthService, private router: Router) { }

  public onSubmit(): void {
    this.errorMessage = ''; // Clear previous errors
    this.authService.login(this.model).subscribe({
      next: () => {
        // On success, navigate to a dashboard or home page
        this.router.navigate(['/dashboard']); // Assume a dashboard page exists
      },
      error: (err) => {
        // Your backend returns 401 Unauthorized for bad credentials
        if (err.status === 401) {
          this.errorMessage = 'Invalid username, email, or password.';
        } else {
          this.errorMessage = 'An unexpected error occurred. Please try again.';
        }
        console.error(err);
      }
    });
  }
}
