import {Component, OnInit} from "@angular/core";
import {KENDO_SCHEDULER, SchedulerEvent,} from "@progress/kendo-angular-scheduler";
import {UserService} from '../../../shared/services/user.service';
import {EventServiceService} from '../../../shared/services/event-service.service';

interface scheduleData {
  id: number;
  user1Name: string;
  user2Name: string;
  user1Surname: string;
  user2Surname: string;
  startTime: Date;
  endTime: Date;
  roomId: number;
}

@Component({
  selector: 'app-calendar-wrapper',
  imports: [KENDO_SCHEDULER],
  templateUrl: './calendar-wrapper.component.html',
  styleUrl: './calendar-wrapper.component.scss'
})
export class CalendarWrapperComponent implements OnInit {

  constructor(private userService: UserService, private eventService: EventServiceService) {
  }

  ngOnInit() {
    this.userService.getScheduleData().subscribe({
      next: (res: scheduleData[]) => {
        this.events = res.map(event => (
          {
            id: event.id,
            start: new Date(event.startTime),
            end: new Date(event.endTime),
            title: `${event.user1Name} ${event.user1Surname} - ${event.user2Name} ${event.user2Surname}`,
          } as SchedulerEvent
        ))

        console.log(this.events)
      },
      error: err => {
        console.log(err)
      }
    })

    this.eventService.getDownloadeScheduleSubscribtion().subscribe((value) => {

      this.userService.getScheduleData().subscribe({
        next: (res: scheduleData[]) => {
          this.events = res.map(event => (
            {
              id: event.id,
              start: new Date(event.startTime),
              end: new Date(event.endTime),
              title: `${event.user1Name} ${event.user1Surname} - ${event.user2Name} ${event.user2Surname} - room ${event.roomId}`,
            } as SchedulerEvent
          ))

          console.log(this.events)
        },
        error: err => {
          console.log(err)
        }
      })

    });

  }

  public selectedDate: Date = new Date();
  public events: SchedulerEvent[] = []


}
