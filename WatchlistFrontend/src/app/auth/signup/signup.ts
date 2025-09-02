import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { MaterialModule } from '../../material.module'; // <-- Import MaterialModule
import { AuthService } from '../../services/auth';
import { RegisterDto } from '../../models/auth';

@Component({
  selector: 'app-signup',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, MaterialModule], // <-- Add modules
  templateUrl: './signup.html',
  styleUrls: ['../auth.css'] // <-- Point to shared stylesheet
})
export class SignupComponent {
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
    this.errorMessage = '';
    this.authService.register(this.model).subscribe({
      next: () => {
        alert('Registration successful! Please log in.'); // This can be replaced with MatSnackBar later
        this.router.navigate(['/login']);
      },
      error: (err) => {
        if (err.status === 409) {
          this.errorMessage = 'Username or Email already exists.';
        } else {
          this.errorMessage = 'An unexpected error occurred. Please try again.';
        }
        console.error(err);
      }
    });
  }
}
