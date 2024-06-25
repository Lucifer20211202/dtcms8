<template>
	<div class="wrapper padb-30">
		<div class="location">
			<el-breadcrumb :separator-icon="ElIconArrowRight">
				<el-breadcrumb-item>当前所在位置</el-breadcrumb-item>
				<el-breadcrumb-item>资源下载</el-breadcrumb-item>
				<el-breadcrumb-item>内容详情</el-breadcrumb-item>
			</el-breadcrumb>
		</div>
		
		<el-row :gutter="20" justify="space-between">
			<el-col :xs="24" :sm="16" :md="18">
				<!--详情-->
				<channel-page-detail>
					<template #default="data">
						<div v-if="data.isComment">
							<div class="tabs-nav-item">
								<span class="title">相关评论</span>
							</div>
							<channel-comment-page :articleId="data.articleId" :pageSize="10" />
						</div>
					</template>
				</channel-page-detail>
			</el-col>
			<el-col :xs="24" :sm="8" :md="6">
				<!--最热文章-->
				<div class="radius-box pad-20">
					<channel-list-bar-top mode="list" to="/down/show/{0}" channel="down" :top="10" orderBy="-Click" />
				</div>
				<div class="bar-title mart-20">
					<div class="title">相关下载</div>
				</div>
				<div class="radius-box mart-20 pad-20">
					<channel-list-bar-rel mode="block" to="/down/show/{0}" :top="10" />
				</div>
			</el-col>
		</el-row>
	</div>
</template>