import { HttpClient } from '@angular/common/http';
import { Injectable, PLATFORM_ID, inject } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { Observable, catchError, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ChatRequest, ChatResponse } from '../models/portfolio.models';

@Injectable({ providedIn: 'root' })
export class ChatApiService {
  private readonly http = inject(HttpClient);
  private readonly platformId = inject(PLATFORM_ID);
  private readonly base = `${environment.apiBaseUrl}/chat`;

  getOrCreateSessionId(): string {
    if (!isPlatformBrowser(this.platformId)) return crypto.randomUUID();

    const key = 'portfolio_chat_session';
    let id = sessionStorage.getItem(key);
    if (!id) {
      id = crypto.randomUUID();
      sessionStorage.setItem(key, id);
    }
    return id;
  }

  send(userMessage: string): Observable<ChatResponse> {
    const payload: ChatRequest = {
      sessionId: this.getOrCreateSessionId(),
      userMessage,
    };

    return this.http.post<ChatResponse>(this.base, payload).pipe(
      catchError(() =>
        throwError(() => new Error('AI assistant is temporarily unavailable.'))
      )
    );
  }
}
