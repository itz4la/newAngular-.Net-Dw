import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { DateFilter } from '../models/analytics.models';

/**
 * Shared state service that holds the current date filter.
 * All chart components subscribe to `dateFilter$` and re-fetch data when it changes.
 */
@Injectable({ providedIn: 'root' })
export class DateFilterService {
  private dateFilterSubject = new BehaviorSubject<DateFilter>({ from: null, to: null });

  /** Observable that emits whenever the date filter changes. */
  dateFilter$: Observable<DateFilter> = this.dateFilterSubject.asObservable();

  /** Get the current snapshot value. */
  get currentFilter(): DateFilter {
    return this.dateFilterSubject.value;
  }

  /** Update the date filter (triggers re-fetch in all charts). */
  setDateFilter(filter: DateFilter): void {
    this.dateFilterSubject.next(filter);
  }

  /** Reset to no filter (all dates). */
  clearFilter(): void {
    this.dateFilterSubject.next({ from: null, to: null });
  }
}
