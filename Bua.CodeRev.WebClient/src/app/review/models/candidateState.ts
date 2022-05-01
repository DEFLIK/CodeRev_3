export enum CandidateState {
    'done' = 'Выполнено',
    'inProcess' = 'В просессе',
    'notStarted' = 'Не начато',
    'skiped' = 'Не выполнено'
}

export type CandidateStateKeys = keyof typeof CandidateState