import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import {AbstractControl,FormBuilder,FormGroup,ReactiveFormsModule,ValidationErrors,Validators} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../Service/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule,ReactiveFormsModule,RouterLink],
 styleUrl: './register.component.css',
 templateUrl: './register.component.html',
})
export class RegisterComponent {

  registerForm: FormGroup;
  submitted = false;
  showPassword = false;
  showConfirmPassword = false;
  loading = false;
  errorMessage = '';
  successMessage = '';
  constructor(private fb: FormBuilder,private authService: AuthService,private router: Router) {
    this.registerForm = this.fb.group({
      fullName: ['',[Validators.required, Validators.minLength(3)]],
      email: ['',[Validators.required, Validators.email]],
      mobile: ['',[Validators.required,Validators.pattern(/^[0-9]{10}$/)]],
      password: ['',[Validators.required,Validators.minLength(8),Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).+$/)]],
      confirmPassword: ['',[Validators.required]],acceptTerms: [false,[Validators.requiredTrue]]
    }, {validators: this.passwordMatchValidator});
  }

  get f() {
    return this.registerForm.controls;
  }

  passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
    const password = control.get('password')?.value;
    const confirmPassword =control.get('confirmPassword')?.value;
    if (password !== confirmPassword) {
      return { 
        passwordMismatch: true 
      };
    }
    return null;
  }

  onRegister(): void {
    this.submitted = true;
    this.errorMessage = '';
    this.successMessage = '';

    if (this.registerForm.invalid || this.loading) {
      this.registerForm.markAllAsTouched();
      return;
    }
    this.loading = true;
    const registerData = {
      fullName: this.f['fullName'].value,
      email: this.f['email'].value,
      mobile: this.f['mobile'].value,
      password: this.f['password'].value
    };

    this.authService.register(registerData).subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success === false) {
          alert(response.message);
          return;
        }
        if (response.success === true) {
          this.successMessage =
            response.message || 'Account created successfully!';
          setTimeout(() => {
            this.router.navigate(['/login']);
          }, 1200);
        }
      },
      error: (error) => {
        this.loading = false;
        alert(error.error?.message || 'Registration failed.');
      }
    });
  }
}
