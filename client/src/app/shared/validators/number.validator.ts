import { AbstractControl, ValidationErrors } from '@angular/forms';

export class NumberValidator {

    static onlyNumbers(control: AbstractControl): ValidationErrors | null {

        if (!/^[0-9]+$/.test(control.value)) {
            return { onlyNumbers: true };
        }

        return null;
    }

}
