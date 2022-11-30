export enum CandidateState {
    'checked' = 'Проверено',
    'toCheck' = 'Нужно проверить',
    'inProcess' = 'В процессе',
    'expired' = 'Не сдано'
}

export type CandidateStateKeys = keyof typeof CandidateState