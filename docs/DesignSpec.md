Design Specification
Overview
This solution implements a bounded course registration feature supporting:

Enrollment
Course drop
FIFO waitlist management
Waitlist promotion
Prerequisite validation
The implementation uses an in-memory data store and focuses only on the requested workflow.

Service Boundary
The CourseRegistrationService is responsible for:

Enrolling learners
Dropping learners
Maintaining enrollment capacity
Managing waitlists
Promoting learners from waitlists
Out of Scope:

Authentication
Authorization
Course catalog management
Payments
Scheduling
Database persistence
Frontend UI
Data Model
Student
Properties:

Id
CompletedCourses
CourseOffering
Properties:

CourseId
RequiredCourse
Capacity
EnrolledStudents
Waitlist
Enrollment Rules
A learner may enroll when:

The learner exists.
The prerequisite course has been completed.
The learner is not already enrolled.
Capacity is available.
If capacity is unavailable, the learner is added to the waitlist.

Prerequisite Policy
A learner must have completed:

INTRO101

before enrolling in the course offering.

Learners who fail prerequisite validation are not enrolled.

Waitlist Policy
FIFO ordering is maintained using Queue.

When a seat becomes available:

The first learner in the waitlist is evaluated.
If eligible, the learner is promoted.
If ineligible, the learner is skipped.
Evaluation continues until an eligible learner is found.
This prevents ineligible learners from blocking promotion.

System Invariants
The following conditions are always maintained:

Enrollment count never exceeds capacity.
Duplicate enrollments are prevented.
Duplicate waitlist entries are prevented.
FIFO waitlist ordering is preserved.
Only eligible learners may be enrolled.
Concurrency Strategy
Enrollment and drop operations use a lock object to ensure thread safety.

This prevents race conditions during:

Capacity checks
Enrollment updates
Waitlist promotions
Tradeoffs
The solution uses an in-memory repository for simplicity.

For production usage:

Persistent storage would be required.
Transactions would be required.
Distributed locking might be necessary in multi-instance deployments.
