import { UserRole } from 'src/app/auth/models/userRole';

export class User {
    constructor(
        public fullName: string,
        public role: UserRole,
        public phoneNumber: string,
        public email: string) {
    }
}