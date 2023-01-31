import ApiAuthorzationRoutes from './components/api-authorization/ApiAuthorizationRoutes';
import { WeeklySchedule } from './components/WeeklySchedule';
import { Home } from "./components/Home";
import { ManageGroups } from './components/ManageGroups';
import { ManageSubjects } from './components/ManageSubjects';
import { ManageTeachers } from './components/ManageTeachers';
import { ManageTrainingTypes } from './components/ManageTrainingTypes';
import { ManageMarkValues } from './components/ManageMarkValues';
import { ManageStudents } from './components/ManageStudents';
import { ManageTrainings } from './components/ManageTrainings';
import { ManageMarks } from './components/ManageMarks';

const AppRoutes = [
  {
    index: true,
    element: <Home />
  },
  {
    path: '/groups-schedule',
    element: <WeeklySchedule key='groups' isGroup={true} />
  },
  {
    path: '/teachers-schedule',
    element: <WeeklySchedule key='teachers' isGroup={false} />
  },
  {
    path: '/manage-groups',
    requireAuth: true,
    element: <ManageGroups />
  },
  {
    path: '/manage-subjects',
    requireAuth: true,
    element: <ManageSubjects />
  },
  {
    path: '/manage-teachers',
    requireAuth: true,
    element: <ManageTeachers />
  },
  {
    path: '/manage-training-types',
    requireAuth: true,
    element: <ManageTrainingTypes />
  },
  {
    path: '/manage-mark-values',
    requireAuth: true,
    element: <ManageMarkValues />
  },
  {
    path: '/manage-students',
    requireAuth: true,
    element: <ManageStudents />
  },
  {
    path: '/manage-trainings',
    requireAuth: true,
    element: <ManageTrainings />
  },
  {
    path: '/manage-marks',
    requireAuth: true,
    element: <ManageMarks />
  },
  ...ApiAuthorzationRoutes
];

export default AppRoutes;
