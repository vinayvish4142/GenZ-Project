
import { Component, OnInit, PLATFORM_ID, inject } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { School } from '../models/models/school.model';
import { Student } from '../models/models/student.model';
import { Exam } from '../models/models/exam.model';
import { AnswerKey, AnswerKeyItem } from '../models/models/answer-key.model';
import { ExamResult } from '../models/models/exam-result.model';
@Component({
  selector: 'app-root',
  standalone: false,
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {

  private readonly KEY = 'gof_desktop_data_v1';
  private readonly PINKEY = 'gof_desktop_pin_v1';
  private readonly platformId = inject(PLATFORM_ID);

  title = 'GenZ';

  page: string = 'dashboard';
  query: string = '';
  feeFilter: string = 'all';

  // PIN Management
  isPinSet = false;
  isUnlocked = false;
  pinInput1 = '';
  pinInput2 = '';
  pinLoginInput = '';

  // Data Store
  schools: School[] = [];
  students: Student[] = [];
  exams: Exam[] = [];
  results: ExamResult[] = [];
  answerKeys: AnswerKey[] = [];

  meta = {
    session: '2026–27',
    foundation: 'GENZ Olympiad Foundation',
    phone: '+91 9835341800 / +91 9711417191'
  };

  // Answer Key
  answerExamId = '';
  manualStudentId = '';
  manualAnswers: { [qNo: number]: string } = {};

  // Toast
  toastMessage = '';
  showToast = false;

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      this.checkPinStatus();
      this.loadData();
    }
  }

  // PIN Management
  checkPinStatus(): void {
    if (!isPlatformBrowser(this.platformId)) return;

    this.isPinSet = !!localStorage.getItem(this.PINKEY);
    this.isUnlocked = sessionStorage.getItem('gof_ok') === 'true';
  }

  createPin(): void {
    if (!isPlatformBrowser(this.platformId)) return;

    if (
      this.pinInput1.length >= 4 &&
      this.pinInput1 === this.pinInput2
    ) {
      localStorage.setItem(this.PINKEY, this.pinInput1);
      sessionStorage.setItem('gof_ok', 'true');

      this.isPinSet = true;
      this.isUnlocked = true;

      this.triggerToast('PIN created successfully!');
    } else {
      alert('PINs do not match or are shorter than 4 digits.');
    }
  }

  unlock(): void {
    if (!isPlatformBrowser(this.platformId)) return;

    const savedPin = localStorage.getItem(this.PINKEY);

    if (savedPin !== null && this.pinLoginInput === savedPin) {
      sessionStorage.setItem('gof_ok', 'true');
      this.isUnlocked = true;
      this.triggerToast('Unlocked successfully!');
    } else {
      alert('Incorrect PIN!');
    }
  }

  // Load Data
  loadData(): void {
    if (!isPlatformBrowser(this.platformId)) return;

    try {
      const raw = localStorage.getItem(this.KEY);

      if (raw) {
        const parsed = JSON.parse(raw);

        this.schools = parsed.schools || [];
        this.students = parsed.students || [];
        this.exams = parsed.exams || [];
        this.results = parsed.results || [];
        this.answerKeys = parsed.answerKeys || [];

        if (parsed.meta) {
          this.meta = { ...this.meta, ...parsed.meta };
        }
      }

      if (this.exams.length && !this.answerExamId) {
        this.answerExamId = this.exams[0].id;
      }

    } catch (error) {
      console.error('Error loading data:', error);
    }
  }

  // Save Data
  save(): void {
    if (!isPlatformBrowser(this.platformId)) return;

    try {
      const payload = {
        schools: this.schools,
        students: this.students,
        exams: this.exams,
        results: this.results,
        answerKeys: this.answerKeys,
        meta: this.meta
      };

      localStorage.setItem(this.KEY, JSON.stringify(payload));

    } catch (error) {
      console.error('Error saving data:', error);
      alert('Could not save changes locally.');
    }
  }

  // Navigation
  setPage(page: string): void {
    this.page = page;
    this.query = '';
  }

  // Toast Notification
  triggerToast(message: string): void {
    this.toastMessage = message;
    this.showToast = true;

    setTimeout(() => {
      this.showToast = false;
    }, 2600);
  }

  // Dashboard Calculations
  get paidCount(): number {
    return this.students.filter(
      s => s.feeStatus === 'Paid'
    ).length;
  }

  get unpaidCount(): number {
    return this.students.length - this.paidCount;
  }

  get averageScore(): string {
    if (!this.results.length) return '—';

    const sum = this.results.reduce((acc, result) => {
      return acc +
        (100 * Number(result.marks) /
          Math.max(1, Number(result.total)));
    }, 0);

    return Math.round(sum / this.results.length) + '%';
  }

  // Helper Methods
  getSchoolName(schoolId: string): string {
    return this.schools.find(
      s => s.id === schoolId
    )?.name || '—';
  }

  getStudentName(studentId: string): string {
    return this.students.find(
      s => s.id === studentId
    )?.name || '—';
  }

  getExamTitle(examId: string): string {
    return this.exams.find(
      e => e.id === examId
    )?.title || '—';
  }

  money(amount: number): string {
    return '₹' + (Number(amount) || 0).toLocaleString('en-IN');
  }

  // Search Filters
  get filteredSchools(): School[] {
    const q = this.query.toLowerCase();

    return this.schools.filter(s =>
      (
        s.name + ' ' +
        s.code + ' ' +
        s.contact + ' ' +
        s.address
      ).toLowerCase().includes(q)
    );
  }

  get filteredStudents(): Student[] {
    const q = this.query.toLowerCase();

    return this.students.filter(s =>
      (
        s.name + ' ' +
        s.father + ' ' +
        s.admission + ' ' +
        (s.phone || '') + ' ' +
        s.grade + ' ' +
        this.getSchoolName(s.schoolId)
      ).toLowerCase().includes(q)
    );
  }

  get filteredFees(): Student[] {
    const q = this.query.toLowerCase();

    return this.students.filter(s =>
      (
        this.feeFilter === 'all' ||
        s.feeStatus === this.feeFilter
      ) &&
      (
        s.name + ' ' +
        s.admission + ' ' +
        this.getSchoolName(s.schoolId)
      ).toLowerCase().includes(q)
    );
  }

  // Answer Key
  get selectedAnswerKeyItems(): AnswerKeyItem[] {
    const key = this.answerKeys.find(
      k => k.examId === this.answerExamId
    );

    return key
      ? [...key.items].sort(
          (a, b) => a.questionNo - b.questionNo
        )
      : [];
  }

  get currentExam(): Exam | undefined {
    return this.exams.find(
      e => e.id === this.answerExamId
    );
  }

  get eligibleStudentsForManual(): Student[] {
    const exam = this.currentExam;

    if (!exam) return [];

    return this.students.filter(
      s => s.grade === exam.grade
    );
  }

  // Update Fee Status
  setFeeStatus(
    studentId: string,
    status: 'Paid' | 'Unpaid'
  ): void {
    const student = this.students.find(
      s => s.id === studentId
    );

    if (student) {
      student.feeStatus = status;
      this.save();
      this.triggerToast(`Fee status updated to ${status}`);
    }
  }

  // Manual Answers
  setManualAnswer(qNo: number, value: string): void {
    this.manualAnswers[qNo] = value;
  }

  // JSON Backup
  backupJSON(): void {
    if (!isPlatformBrowser(this.platformId)) return;

    const data = localStorage.getItem(this.KEY) || '{}';

    const blob = new Blob(
      [data],
      { type: 'application/json' }
    );

    const url = URL.createObjectURL(blob);

    const link = document.createElement('a');
    link.href = url;
    link.download =
      `GOF_Backup_${new Date().toISOString().slice(0, 10)}.json`;

    document.body.appendChild(link);
    link.click();
    link.remove();

    URL.revokeObjectURL(url);
  }
}
