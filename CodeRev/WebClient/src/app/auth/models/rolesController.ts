import { UserRole } from './userRole';

export class RolesController {
    public static reviewRoles: UserRole[] = [
        UserRole.admin,
        UserRole.interviewer,
        UserRole.hrManager
    ];

    public static contestRoles: UserRole[] = [
        UserRole.candidate
    ];

    public static getDefaultRoot(role: UserRole): string {
        if (this.reviewRoles.includes(role)) {
            return '/review';
        } else if (this.contestRoles.includes(role)) {
            return '/contest';
        }

        return '';
    }
}