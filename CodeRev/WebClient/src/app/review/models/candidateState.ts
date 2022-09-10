export enum CandidateState {
    'done' = 'Выполнено',
    'inProcess' = 'В процессе',
    'notStarted' = 'Не начато',
    'skiped' = 'Истёк срок'
}

export type CandidateStateKeys = keyof typeof CandidateState