import { ValidatorFn, AbstractControl, ValidationErrors } from "@angular/forms";

export function emailValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const value = control.value;

    // Allow empty fields
    if (!value) {
      return null;
    }

    // Check if the value matches the email pattern
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    const isValidEmail = emailRegex.test(value);

    return isValidEmail ? null : { invalidEmail: true };
  };
}
