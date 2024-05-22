import {Directive} from '@angular/core';
import {Observable, Subject, takeUntil} from 'rxjs';
import {SensorDto} from "../../../models/Entities";

@Directive()
export class BaseGraphComponent {
  idFromRoute: number | undefined;
  chartOptions: any = {};
  public activeOptionButton = "1d";
  public activeTimeInterval?: number;
  public currentReadingType: string = "temperature";

  protected unsubscribe$ = new Subject<void>();

  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  initChart(): void {
    this.chartOptions = {
      series: [{data: []}],
      chart: {
        type: "area",
        height: 300
      },
      dataLabels: {
        enabled: false
      },
      markers: {
        size: 0
      },
      xaxis: {
        type: "datetime",
        tickAmount: 6
      },
      yaxis: {
        labels: {
          formatter: function (value: number) {
            return value.toFixed(1);
          }
        }
      },
      tooltip: {
        x: {
          format: "hh:mm  dd-MMM yyyy"
        }
      },
      fill: {
        type: "gradient",
        gradient: {
          shadeIntensity: 1,
          opacityFrom: 0.7,
          opacityTo: 0.9,
          stops: [0, 100]
        }
      }
    };
  }

  subscribeToReadings(observable: Observable<Record<number, SensorDto[]> | undefined>, seriesName: string) {
    observable
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(readings => {
        if (readings) {
          const series = this.chartOptions.series.find((s: any) => s.name === seriesName);
          const data = readings[this.idFromRoute!];
          if (data && data.length > 0) {
            const newSeries = data.map((reading: SensorDto) => ({
              x: new Date(reading.TimeStamp).getTime(),
              y: reading.Value
            }));

            if (series) {
              series.data = newSeries;
            } else {
              this.chartOptions.series.push({name: seriesName, data: newSeries});
            }
          }
        }
      });
  }

  submitToHistory(observable: Observable<Record<number, SensorDto[]> | undefined>, minTime: number, maxTime: number) {
    observable
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(readings => {
        if (readings && readings[this.idFromRoute!]) {
          this.chartOptions = {
            ...this.chartOptions,
            xaxis: {
              ...this.chartOptions.xaxis,
              min: minTime,
              max: maxTime
            }
          };
        }
      });
  }
}
