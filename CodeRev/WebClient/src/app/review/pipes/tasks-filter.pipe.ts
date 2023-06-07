import { Pipe, PipeTransform } from '@angular/core';
import { TaskInfo } from '../models/taskInfo';

@Pipe({
    name: 'tasksFilter'
})
export class TasksFilterPipe implements PipeTransform {

    public transform(value: TaskInfo[], searchCriteria: string): TaskInfo[] {
        return value
            .filter((task: TaskInfo) => task.name.toLowerCase().includes(searchCriteria.toLowerCase()))
            .sort((task, next) => ('' + task.name).localeCompare(next.name));
    }

}
