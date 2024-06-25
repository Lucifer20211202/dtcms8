<template>
	<div class="wrapper">
		<div class="location">
			<slot name="location" />
		</div>
		<el-row :gutter="20" justify="space-between" class="account-box">
			<el-col :xs="24" :sm="8" :md="6">
				<AccountNavbar />
			</el-col>
			<el-col :xs="24" :sm="16" :md="18">
				<div class="container">
					<slot name="default" />
				</div>
			</el-col>
		</el-row>
	</div>
</template>