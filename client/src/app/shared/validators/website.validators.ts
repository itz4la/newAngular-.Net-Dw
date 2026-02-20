import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";

export function websiteValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
      const value = control.value;

      // Allow empty fields
      if (!value) {
        return null;
      }
    const urlRegex = /^(https?:\/\/)?([\w-]+\.)+[\w-]+(\/[\w-]*)*$/i; // Basic URL pattern
    const isValidWebsite = urlRegex.test(control.value);
    return isValidWebsite ? null : { invalidWebsite: true };
  };
}
