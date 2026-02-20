import { ValidatorFn, AbstractControl, ValidationErrors } from "@angular/forms";

export function cannotContainSpace(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (typeof control.value === 'string' && control.value.includes(' ')) {
      return { cannotContainSpace: true };
    }
    return null;
  };
}
