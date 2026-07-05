import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, catchError, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { SkillGroupApiDto } from '../models/portfolio.models';

@Injectable({ providedIn: 'root' })
export class SkillsApiService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiBaseUrl}/skills`;

  getAll(): Observable<SkillGroupApiDto[]> {
    return this.http.get<SkillGroupApiDto[]>(this.base).pipe(
      catchError(() => throwError(() => new Error('Failed to load skills.')))
    );
  }
}
