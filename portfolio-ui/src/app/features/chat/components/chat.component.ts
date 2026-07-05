import {
  AfterViewChecked,
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  OnInit,
  PLATFORM_ID,
  inject,
  signal,
  viewChild,
  computed,
} from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ChatApiService } from '../../../core/services/chat-api.service';
import { ToastService }   from '../../../core/services/toast.service';

interface ChatMessage {
  id:        string;
  role:      'user' | 'assistant';
  content:   string;
  timestamp: Date;
}

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './chat.component.html',
  styleUrl:    './chat.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ChatComponent implements OnInit, AfterViewChecked {
  private readonly chatApi    = inject(ChatApiService);
  private readonly toastSvc   = inject(ToastService);
  private readonly platformId = inject(PLATFORM_ID);

  readonly messagesRef = viewChild<ElementRef<HTMLElement>>('messagesContainer');

  // ── UI state ───────────────────────────────────────────────────────
  readonly isOpen     = signal(false);
  readonly isTyping   = signal(false);
  readonly inputText  = signal('');
  readonly messages   = signal<ChatMessage[]>([]);
  readonly hasUnread  = signal(false);

  readonly isEmpty = computed(() => this.messages().length === 0);

  private shouldScrollToBottom = false;

  // ── Suggested starter prompts ──────────────────────────────────────
  readonly suggestions = [
    'What projects have you built?',
    'What is your tech stack?',
    'Are you available for hire?',
    'Tell me about NexusAgent.',
  ];

  ngOnInit(): void {
    // Show welcome message on first open
    if (isPlatformBrowser(this.platformId)) {
      setTimeout(() => {
        this.addAssistantMessage(
          "Hi! I'm your AI assistant. Ask me anything about this portfolio — projects, skills, availability, or anything else you'd like to know. 👋"
        );
      }, 400);
    }
  }

  ngAfterViewChecked(): void {
    if (this.shouldScrollToBottom) {
      this.scrollToBottom();
      this.shouldScrollToBottom = false;
    }
  }

  // ── Panel toggle ───────────────────────────────────────────────────
  toggleChat(): void {
    this.isOpen.update(v => !v);
    if (this.isOpen()) {
      this.hasUnread.set(false);
      this.shouldScrollToBottom = true;
    }
  }

  closeChat(): void {
    this.isOpen.set(false);
  }

  // ── Send message ───────────────────────────────────────────────────
  sendMessage(text?: string): void {
    const content = (text ?? this.inputText()).trim();
    if (!content || this.isTyping()) return;

    this.inputText.set('');
    this.addUserMessage(content);
    this.isTyping.set(true);
    this.shouldScrollToBottom = true;

    this.chatApi.send(content).subscribe({
      next: (res) => {
        this.isTyping.set(false);
        this.addAssistantMessage(res.assistantReply);
        if (!this.isOpen()) this.hasUnread.set(true);
      },
      error: (err: Error) => {
        this.isTyping.set(false);
        this.addAssistantMessage(
          "I'm having trouble connecting right now. Please try again in a moment."
        );
        this.toastSvc.error('AI unavailable', err.message);
      },
    });
  }

  useSuggestion(text: string): void {
    this.sendMessage(text);
  }

  onKeyDown(event: KeyboardEvent): void {
    if (event.key === 'Enter' && !event.shiftKey) {
      event.preventDefault();
      this.sendMessage();
    }
  }

  updateInput(value: string): void {
    this.inputText.set(value);
  }

  // ── Helpers ────────────────────────────────────────────────────────
  private addUserMessage(content: string): void {
    this.messages.update(msgs => [
      ...msgs,
      { id: crypto.randomUUID(), role: 'user', content, timestamp: new Date() },
    ]);
    this.shouldScrollToBottom = true;
  }

  private addAssistantMessage(content: string): void {
    this.messages.update(msgs => [
      ...msgs,
      { id: crypto.randomUUID(), role: 'assistant', content, timestamp: new Date() },
    ]);
    this.shouldScrollToBottom = true;
  }

  private scrollToBottom(): void {
    const el = this.messagesRef()?.nativeElement;
    if (el) el.scrollTop = el.scrollHeight;
  }

  formatTime(date: Date): string {
    return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
  }

  trackMessage(_: number, msg: ChatMessage): string {
    return msg.id;
  }
}
