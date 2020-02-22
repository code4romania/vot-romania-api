import {
  Component,
  OnInit,
  ViewChild,
  ElementRef,
  AfterViewInit
} from "@angular/core";
declare var H: any;

@Component({
  selector: "app-polling-station-search",
  templateUrl: "./polling-station-search.component.html",
  styleUrls: ["./polling-station-search.component.scss"]
})
export class PollingStationSearchComponent implements OnInit, AfterViewInit {
  searchText: string =
    "Caută adresa ta pentru a afla la ce secție ești arondat";

  private platform: any;

  @ViewChild("map", { static: true })
  public mapElement: ElementRef;

  constructor() {
    this.platform = new H.service.Platform({
      apikey: "Um0LhLV4phI2QpCYrBCwmWgvdjmH6NFvd709PhMqsQg"
    });
  }

  ngOnInit() {}
  ngAfterViewInit(): void {
    var defaultLayers = this.platform.createDefaultLayers();

    var map = new H.Map(
      this.mapElement.nativeElement,
      defaultLayers.vector.normal.map,
      {
        center: { lat: 45.658, lng: 25.6012 },
        zoom: 7,
        pixelRatio: window.devicePixelRatio || 1
      }
    );

    var behavior = new H.mapevents.Behavior(new H.mapevents.MapEvents(map));

  }
}
