export interface Student {
  id: string;
  admission: string;
  name: string;
  father: string;
  phone: string;
  grade: string;
  schoolId: string;
  subject: string;
  feeStatus: 'Paid' | 'Unpaid';
  photo?: string;
}