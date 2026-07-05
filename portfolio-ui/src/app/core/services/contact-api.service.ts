import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, catchError, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { SendMessageResponse } from '../models/portfolio.models';

export interface ContactPayload {
  name: string;
  email: string;
  subject: string;
  message: string;
}

@Injectable({ providedIn: 'root' })
export class ContactApiService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiBaseUrl}/contact`;

  send(payload: ContactPayload): Observable<SendMessageResponse> {
    return this.http.post<SendMessageResponse>(this.base, payload).pipe(
      catchError(err => {
        const msg = err?.error?.error ?? 'Failed to send message. Please try again.';
        return throwError(() => new Error(msg));
      })
    );
  }
}
