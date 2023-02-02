import { Component, OnInit } from '@angular/core';
import { AsyncValidatorFn, FormBuilder, Validators, AbstractControl } from '@angular/forms';
import { Router } from '@angular/router';
import { AccountService } from '../account.service';
import { finalize, map } from 'rxjs';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
})
export class RegisterComponent {
  constructor(
    private fb: FormBuilder,
    private accountService: AccountService,
    private router: Router
  ) {}

  errors : string[] | null = null;

  complexPasswordPattern =
    "(?=^.{6,10}$)(?=.*d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&amp;*()_+}{&quot;:;'?/&gt;.&lt;,])(?!.*s).*$";

  registerForm = this.fb.group({
    displayName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email], [this.validateEmailNotTaken()]],
    password: [
      '',
      [Validators.required, Validators.pattern(this.complexPasswordPattern)],
    ],
  });

  onSubmit() {
    this.accountService.register(this.registerForm.value).subscribe({
      next: () => this.router.navigateByUrl('/shop'),
      error: (error) => this.errors = error.errors
    });
  }

  validateEmailNotTaken() : AsyncValidatorFn {

    return (control: AbstractControl) => {
      return this.accountService.checkEmailExists(control.value).pipe(
        map((result) => result ? {emailExists: true} : null),
        finalize(() => control.markAsTouched())
      )
    }

  }

}
