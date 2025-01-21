import {Injectable} from '@angular/core';
import {Observable, Subject} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class EventServiceService {

  private openAddMeetingDialogSubscribtion = new Subject<boolean>();
  private openAddAvailabilityDialogSubscribtion = new Subject<boolean>();
  private logInEvent = new Subject<boolean>();
  private downloadeScheduleSubscribtion = new Subject<boolean>();

  public getOpenAddMeetingDialogSubscribtion(): Observable<boolean> {
    return this.openAddMeetingDialogSubscribtion.asObservable();
  }

  public triggerOpenAddMeetingDialogButton() {
    this.openAddMeetingDialogSubscribtion.next(true);
  }

  public getAddAvailabilityDialogSubscribtion(): Observable<boolean> {
    return this.openAddAvailabilityDialogSubscribtion.asObservable();
  }

  public triggerAddAvailabilityDialogButton() {
    this.openAddAvailabilityDialogSubscribtion.next(true);
  }

  public getLogInEvent(): Observable<boolean> {
    return this.logInEvent.asObservable();
  }

  public triggerLogInEvent() {
    this.logInEvent.next(true);
  }

  public getDownloadeScheduleSubscribtion(): Observable<boolean> {
    return this.downloadeScheduleSubscribtion.asObservable();
  }

  public triggerDownloadeScheduleSubscribtion(): void {
    this.downloadeScheduleSubscribtion.next(true);
  }
}
