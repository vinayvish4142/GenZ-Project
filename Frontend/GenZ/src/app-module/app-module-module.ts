import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';

import { App } from '../app/app';
import { routes } from '../app/app.routes';

import { LoginComponent } from '../app/components/login/login.component';
import { RegisterComponent } from '../app/components/register/register.component';

@NgModule({
  declarations: [
    App
  ],
  imports: [
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forRoot(routes),
    LoginComponent,
    RegisterComponent
  ],
  providers: [
    provideHttpClient()
  ],
  bootstrap: [
    App
  ]
})
export class AppModuleModule {}