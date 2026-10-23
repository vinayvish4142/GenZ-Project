export interface AnswerKeyItem {
  questionNo: number;
  questionText: string;
  optionA: string;
  optionB: string;
  optionC: string;
  optionD: string;
  correctAnswer: string;
  marks: number;
  negativeMarks: number;
  solution: string;
}

export interface AnswerKey {
  examId: string;
  items: AnswerKeyItem[];
}