import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { MaterialModule } from '../../material.module'; // <-- Import MaterialModule
import { AuthService } from '../../services/auth.service';
import { LoginDto } from '../../models/auth';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, MaterialModule], // <-- Add modules
  templateUrl: './login.html',
  styleUrls: ['../auth.css'] // <-- Point to shared stylesheet
})
export class LoginComponent {
  public model: LoginDto = {
    usernameOrEmail: '',
    password: ''
  };
  public errorMessage = '';

  constructor(private authService: AuthService, private router: Router) {}

  public onSubmit(): void {
    this.errorMessage = '';
    this.authService.login(this.model).subscribe({
      next: () => {
        this.router.navigate(['/app/dashboard']);
      },
      error: (err) => {
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
