import {Injectable} from '@angular/core';
import {Observable, Subject} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class EventServiceService {

  private openAddMeetingDialogSubscribtion = new Subject<boolean>();
  private openAddAvailabilityDialogSubscribtion = new Subject<boolean>();

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
}
