export enum Step {
  MainMenu = "Main menu",
  ModifyVertex = "Modify vertex",
}

export const step = $state({
  value: Step.MainMenu,
  set(step: Step) {
    this.value = step;
  },
});
