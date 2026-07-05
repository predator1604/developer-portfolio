export interface PersonalInfo {
  label: string;
  value: string;
  isLink?: boolean;
  href?: string;
  highlight?: 'green' | 'accent';
}

export interface StatCard {
  value: string;
  suffix?: string;
  description: string;
}

export interface CodeLine {
  type: 'keyword' | 'variable' | 'string' | 'comment' | 'punctuation' | 'blank';
  parts: CodePart[];
}

export interface CodePart {
  text: string;
  color: 'purple' | 'blue' | 'green' | 'amber' | 'white' | 'muted' | 'plain';
}

export type SkillCategory =
  | 'all'
  | 'backend'
  | 'frontend'
  | 'ai'
  | 'devops'
  | 'database';

export interface Skill {
  name: string;
  level: number;
}

export interface SkillGroup {
  id: string;
  title: string;
  category: Exclude<SkillCategory, 'all'>;
  accentColor: string;
  accentBg: string;
  iconPath: string;
  skills: Skill[];
  tools?: string[];
}

export interface SkillFilter {
  id: SkillCategory;
  label: string;
}

export type ProjectSize = 'featured' | 'compact';
export type ProjectStatus = 'live' | 'in-progress' | 'archived';

export interface ProjectTechChip {
  label: string;
}

export interface ProjectFeature {
  text: string;
}

export interface Project {
  id: string;
  number: string;
  size: ProjectSize;
  title: string;
  category: string;
  description: string;
  feature?: ProjectFeature;
  stack: ProjectTechChip[];
  accentColor: string;
  accentBg: string;
  status: ProjectStatus;
  statusLabel: string;
  liveUrl?: string;
  githubUrl?: string;
}

export interface ContactInfoItem {
  label: string;
  value: string;
  iconPath: string;
}

export interface SocialLink {
  id: string;
  label: string;
  href: string;
  iconPath: string;
}

export interface ContactFormValue {
  name: string;
  email: string;
  subject: string;
  message: string;
}

export type SubmitState = 'idle' | 'submitting' | 'success' | 'error';

export interface FooterNavLink {
  label: string;
  fragment?: string;
  href?: string;
  muted?: boolean;
}

export interface FooterNavColumn {
  title: string;
  links: FooterNavLink[];
}

export interface FooterSocialLink {
  id: string;
  label: string;
  href: string;
  iconPath: string;
}

export interface FooterTechChip {
  label: string;
}

export interface SendMessageResponse {
  messageId: string;
  confirmationText: string;
}

export interface ProjectApiDto {
  id: string;
  title: string;
  slug: string;
  category: string;
  description: string;
  feature?: string;
  stack: string[];
  accentColor: string;
  accentBg: string;
  status: string;
  statusLabel: string;
  liveUrl?: string;
  gitHubUrl?: string;
  sortOrder: number;
}

export interface SkillApiDto {
  name: string;
  level: number;
}

export interface SkillGroupApiDto {
  id: string;
  groupId: string;
  title: string;
  category: string;
  accentColor: string;
  accentBg: string;
  iconPath: string;
  skills: SkillApiDto[];
  tools: string[];
  sortOrder: number;
}

export interface ChatRequest {
  sessionId: string;
  userMessage: string;
}

export interface ChatResponse {
  sessionId: string;
  assistantReply: string;
  history: ChatHistoryMessage[];
}

export interface ChatHistoryMessage {
  role: 'User' | 'Assistant' | 'System';
  content: string;
  timestamp: string;
}

export interface Toast {
  id: string;
  type: 'success' | 'error' | 'info';
  title: string;
  message?: string;
  duration: number;
}
