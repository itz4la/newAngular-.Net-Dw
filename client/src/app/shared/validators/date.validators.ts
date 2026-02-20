import { AbstractControl, ValidatorFn } from '@angular/forms';

export function dateSupérieureAJourActuel(): ValidatorFn {
  return (control: AbstractControl): {[key: string]: any} | null => {
    const dateChoisie = new Date(control.value);
    const dateActuelle = new Date();
    dateActuelle.setHours(0, 0, 0, 0);  
    
    if (dateChoisie.getTime() >= dateActuelle.getTime()) {
      return null; 
    }
    return { 'dateInvalide': true }; 
  };
}
