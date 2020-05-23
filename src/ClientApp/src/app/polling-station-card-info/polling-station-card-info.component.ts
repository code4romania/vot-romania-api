import { Component, Input } from '@angular/core';
import { PollingStation } from '../services/data.service';

@Component({
  selector: 'app-polling-station-card-info',
  templateUrl: './polling-station-card-info.component.html',
  styleUrls: ['./polling-station-card-info.component.scss']
})
export class PollingStationCardInfoComponent {
  isCollapsed = true;
  @Input() lightTheme: boolean;
  @Input() pollingStation: PollingStation;
  @Input() distance: number;
}
