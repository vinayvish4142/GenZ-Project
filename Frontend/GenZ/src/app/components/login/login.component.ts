//  import { Component } from '@angular/core';
// import {FormBuilder,FormGroup,Validators,ReactiveFormsModule} from '@angular/forms';
// import { CommonModule } from '@angular/common';
// import { RouterLink } from '@angular/router';
// @Component({
//   selector: 'app-login',
//   standalone: true,
//   imports: [CommonModule, ReactiveFormsModule,RouterLink],
//   templateUrl: './login.component.html',
//   styleUrl: './login.component.css'
// })
// export class LoginComponent {

//   loginForm: FormGroup;
//   showPassword = false;
//   submitted = false;

//   constructor(private fb: FormBuilder) {
//     this.loginForm = this.fb.group({
//       email: ['', [
//         Validators.required,
//         Validators.email
//       ]],
//       password: ['', [
//         Validators.required,
//         Validators.minLength(6)
//       ]],
//       rememberMe: [false]
//     });
//   }

//   togglePassword() {
//     this.showPassword = !this.showPassword;
//   }

//   onLogin() {
//     this.submitted = true;

//     if (this.loginForm.invalid) {
//       this.loginForm.markAllAsTouched();
//       return;
//     }

//     const { email, rememberMe } = this.loginForm.value;

//     console.log('Login submitted:', {
//       email,
//       rememberMe
//     });

//     // TODO: Call ASP.NET Core login API here.
//     // Do not log or store the password.
//   }
// }

//==============================================================


import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import {FormBuilder,FormGroup,ReactiveFormsModule,Validators} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../Service/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule,ReactiveFormsModule,RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements OnInit {
  loginForm!: FormGroup;
  submitted = false;
  showPassword = false;
  loading = false;
  errorMessage = '';
  successMessage = '';

  constructor(private fb: FormBuilder,private authService: AuthService,private router: Router) {}
  ngOnInit(): void {
    this.loginForm = this.fb.group({
      email: ['',[Validators.required, Validators.email]],
      password: ['',[Validators.required, Validators.minLength(8)]],
      rememberMe: [false]
    });
  }

  togglePassword(): void {
    this.showPassword = !this.showPassword;
  }

  onLogin(): void {
    this.submitted = true;
    this.errorMessage = '';
    if (this.loginForm.invalid || this.loading) {
      this.loginForm.markAllAsTouched();
      return;
    }
    this.loading = true;
    const loginData = {
      email: this.loginForm.value.email,
      password: this.loginForm.value.password
    };

    this.authService.login(loginData).subscribe({
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
