import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject, takeUntil } from 'rxjs';
import { AnalyticsService } from '../services/analytics.service';
import { DateFilterService } from '../services/date-filter.service';

@Component({
  selector: 'app-date-filter',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="flex flex-wrap items-center gap-3">
      <!-- From date -->
      <div class="flex items-center gap-2">
        <label for="dateFrom" class="text-sm font-medium text-slate-500">From</label>
        <input
          id="dateFrom"
          type="date"
          class="date-input"
          [min]="minDate"
          [max]="toDate || maxDate"
          [(ngModel)]="fromDate"
          (ngModelChange)="onDateChange()"
        />
      </div>

      <!-- To date -->
      <div class="flex items-center gap-2">
        <label for="dateTo" class="text-sm font-medium text-slate-500">To</label>
        <input
          id="dateTo"
          type="date"
          class="date-input"
          [min]="fromDate || minDate"
          [max]="maxDate"
          [(ngModel)]="toDate"
          (ngModelChange)="onDateChange()"
        />
      </div>

      <!-- Reset button -->
      @if (fromDate || toDate) {
        <button
          class="reset-btn"
          (click)="resetFilter()"
          title="Reset date filter"
        >
          <svg class="w-4 h-4" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12"/>
          </svg>
          Reset
        </button>
      }

      <!-- Active filter badge -->
      @if (fromDate || toDate) {
        <span class="filter-badge">
          <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" d="M3 4a1 1 0 011-1h16a1 1 0 011 1v2.586a1 1 0 01-.293.707l-6.414 6.414a1 1 0 00-.293.707V17l-4 4v-6.586a1 1 0 00-.293-.707L3.293 7.293A1 1 0 013 6.586V4z"/>
          </svg>
          Filter active
        </span>
      }
    </div>
  `,
  styles: [`
    :host { display: block; }

    .date-input {
      padding: 0.45rem 0.75rem;
      border: 1px solid #e2e8f0;
      border-radius: 0.625rem;
      font-size: 0.8125rem;
      color: #334155;
      background: rgba(255,255,255,0.7);
      backdrop-filter: blur(8px);
      transition: border-color 0.2s, box-shadow 0.2s;
      outline: none;
    }
    .date-input:hover {
      border-color: #c7d2fe;
    }
    .date-input:focus {
      border-color: #6366f1;
      box-shadow: 0 0 0 3px rgba(99,102,241,0.1);
    }

    .reset-btn {
      display: inline-flex;
      align-items: center;
      gap: 0.25rem;
      padding: 0.45rem 0.75rem;
      border: 1px solid #e2e8f0;
      border-radius: 0.625rem;
      font-size: 0.8125rem;
      color: #64748b;
      background: rgba(255,255,255,0.7);
      backdrop-filter: blur(8px);
      cursor: pointer;
      transition: all 0.2s;
    }
    .reset-btn:hover {
      border-color: #f87171;
      color: #ef4444;
      background: rgba(254,226,226,0.5);
    }

    .filter-badge {
      display: inline-flex;
      align-items: center;
      gap: 0.25rem;
      padding: 0.3rem 0.65rem;
      border-radius: 999px;
      font-size: 0.75rem;
      font-weight: 500;
      color: #6366f1;
      background: rgba(238,242,255,0.8);
      border: 1px solid rgba(199,210,254,0.6);
    }
  `]
})
export class DateFilterComponent implements OnInit, OnDestroy {
  minDate = '';
  maxDate = '';
  fromDate = '';
  toDate = '';

  private destroy$ = new Subject<void>();

  constructor(
    private analytics: AnalyticsService,
    private dateFilter: DateFilterService,
  ) {}

  ngOnInit(): void {
    this.analytics.getDateRange().pipe(takeUntil(this.destroy$)).subscribe({
      next: (range) => {
        // Backend returns ISO strings – extract YYYY-MM-DD for input[type=date]
        this.minDate = range.minDate.substring(0, 10);
        this.maxDate = range.maxDate.substring(0, 10);
      },
    });
  }

  onDateChange(): void {
    this.dateFilter.setDateFilter({
      from: this.fromDate || null,
      to: this.toDate || null,
    });
  }

  resetFilter(): void {
    this.fromDate = '';
    this.toDate = '';
    this.dateFilter.clearFilter();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
