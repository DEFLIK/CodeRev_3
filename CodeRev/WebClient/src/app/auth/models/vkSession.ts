export class VkSession
{
    constructor(
        public expire: string,
        public mid: string,
        public secret: string,
        public sid: string,
        public sig: string) {
    }
}