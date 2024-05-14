// base-graph.component.ts
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
            // Format the value as you desire, for example, to show only two decimal places
            return value.toFixed(1); // This will round the value to two decimal places
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

  /* Method to subscribe to the selected reading */
  /* Call by passing the observable and series name as parameters, like this: */
  /* this.subscribeToReading(this.ws.temperatureReadings, 'Temperature') */
  subscribeToReadings(observable: Observable<Record<number, SensorDto[]> | undefined>, seriesName: string) {
    observable
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(readings => {
        if (readings) {
          // Find series to update
          let series = this.chartOptions.series.find((s: any) => s.name === seriesName);

          const data = readings[this.idFromRoute!];
          if (data && data.length > 0) {
            const newSeries = data.map((reading: SensorDto) => ({
              x: new Date(reading.TimeStamp).getTime(), // Convert timestamp to milliseconds
              y: reading.Value
            }));

            // Update the series with new data
            if (series) {
              series.data = newSeries;
            } else {
              series = {name: seriesName, data: newSeries};
              this.chartOptions.series.push(series);
            }
          }
        }
      });
  }

  //used in same way as subscribeToReadings method
  submitToHistory(observable: Observable<Record<number, SensorDto[]> | undefined>, minTime: number, maxTime: number) {
    observable
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(readings => {
        if (!readings)
          return

        if (readings[this.idFromRoute!]) {

          // Update chartOptions with new x-axis range
          this.chartOptions = {
            ...this.chartOptions,
            xaxis: {
              ...this.chartOptions.xaxis,
              min: minTime,
              max: maxTime
            }
          }
        }
      });
  }
}
