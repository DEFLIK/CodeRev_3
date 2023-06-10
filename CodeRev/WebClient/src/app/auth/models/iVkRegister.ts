import { VkSession } from './vkSession';

export interface IVkRegister {
    firstName: string;
    surname: string;
    vkDomainLink: string;
    vkId: string;
    vkSession: VkSession;
}