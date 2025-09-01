import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth';
import { RegisterDto } from '../../models/auth';

import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
@Component({
  selector: 'app-signup',
  templateUrl: './signup.html',
  imports: [
    CommonModule, // For *ngIf
    FormsModule,  // For ngForm and [(ngModel)]
    RouterLink    // For the routerLink directive
  ],
  styleUrls: ['../auth.css']
})
export class SignupComponent {
  // Initialize the DTO object
  public model: RegisterDto = {
    username: '',
    email: '',
    password: '',
    fName: '',
    lName: ''
  };
  public errorMessage = '';

  constructor(private authService: AuthService, private router: Router) { }

  public onSubmit(): void {
    this.errorMessage = ''; // Clear previous errors
    this.authService.register(this.model).subscribe({
      next: () => {
        // On success, redirect to login with a success message
        alert('Registration successful! Please log in.');
        this.router.navigate(['/login']);
      },
      error: (err) => {
        // Check for different error responses from the backend
        if (err.status === 409) { // 409 Conflict
          this.errorMessage = 'Username or Email already exists.';
        } else {
          this.errorMessage = 'An unexpected error occurred. Please try again.';
        }
        console.error(err);
      }
    });
  }
}
