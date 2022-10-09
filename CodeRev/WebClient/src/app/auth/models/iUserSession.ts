import { UserRole } from './userRole';

export class UserSession {
    public aud?: string;
    public email?: string;
    public exp?: string;
    public role?: UserRole;
    public iss?: string;
    public nbf?: string;
    public sub?: string;
}